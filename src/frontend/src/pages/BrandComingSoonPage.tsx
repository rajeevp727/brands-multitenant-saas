import { useMemo } from 'react';
import { Link, useParams } from 'react-router-dom';
import './BrandComingSoonPage.css';

type PreviewBrand = {
  name: string;
  tagline: string;
  category: string;
  launch: string;
  highlights: string[];
};

const PREVIEW_BRANDS: Record<string, PreviewBrand> = {
  vanavajram: {
    name: 'VanaVajram',
    tagline: "Earth's purest gems for conscious luxury buyers.",
    category: 'Ethical Jewelry Marketplace',
    launch: 'Planned in Q3',
    highlights: ['Blockchain provenance cards', 'Conflict-free sourcing', 'Global catalog APIs']
  },
  vajravalli: {
    name: 'VajraValli',
    tagline: 'Curated high-clarity diamonds for modern premium brands.',
    category: 'B2B Diamond Platform',
    launch: 'Planned in Q4',
    highlights: ['Wholesale buyer portal', 'Live quality certification feed', 'AI-assisted assortment matching']
  },
  morebrands: {
    name: 'More Brands',
    tagline: 'Expanding the multi-tenant ecosystem with focused vertical products.',
    category: 'Multi-Brand Expansion',
    launch: 'Rolling launches',
    highlights: ['Reusable auth and tenant core', 'Shared billing and analytics', 'Brand-level design systems']
  }
};

const toTitle = (value: string) => {
  if (!value) return 'Upcoming Brand';
  return value
    .split(/[-_]/g)
    .filter(Boolean)
    .map((part) => part[0].toUpperCase() + part.slice(1))
    .join(' ');
};

export default function BrandComingSoonPage() {
  const { tenantId = '' } = useParams();

  const preview = useMemo(() => {
    const key = tenantId.toLowerCase();
    return PREVIEW_BRANDS[key] ?? {
      name: toTitle(tenantId),
      tagline: 'This brand experience is being prepared for local launch.',
      category: 'Upcoming Brand Workspace',
      launch: 'Coming soon',
      highlights: ['Tenant-aware architecture', 'Dedicated UI and API slots', 'Shared SaaS security baseline']
    };
  }, [tenantId]);

  return (
    <div className="brand-preview-page">
      <section className="brand-preview-card">
        <span className="brand-preview-pill">Coming Soon</span>
        <h1>{preview.name}</h1>
        <p className="brand-preview-tagline">{preview.tagline}</p>
        <p className="brand-preview-meta"><strong>Category:</strong> {preview.category}</p>
        <p className="brand-preview-meta"><strong>Local status:</strong> {preview.launch}</p>

        <div className="brand-preview-list">
          {preview.highlights.map((item) => (
            <div key={item} className="brand-preview-item">{item}</div>
          ))}
        </div>

        <Link to="/" className="brand-preview-link">Back to dashboard</Link>
      </section>
    </div>
  );
}
